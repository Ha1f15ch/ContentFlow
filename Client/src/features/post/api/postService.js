import apiClient from "@/shared/api/HttpClient.js";

export const POST_STATUS = {
  Draft: 0,
  PendingModeration: 1,
  Published: 2,
  Rejected: 3,
  Archived: 4,
};

export const POST_SORT_BY = {
  CreatedAt: 0,
  PublishedAt: 1,
  Title: 2,
  CommentCount: 3,
};

export const SORT_DIRECTION = {
  Asc: 0,
  Desc: 1,
};

function buildPostQueryParams({
  page = 1,
  pageSize = 10,
  filter = {},
} = {}) {
  const params = {
    Page: page,
    PageSize: pageSize,
  };

  if (filter.search) {
    params["Filter.Search"] = filter.search;
  }

  if (filter.categoryId != null) {
    params["Filter.CategoryId"] = filter.categoryId;
  }

  if (filter.status != null) {
    params["Filter.Status"] = filter.status;
  }

  if (filter.authorId != null) {
    params["Filter.AuthorId"] = filter.authorId;
  }

  if (filter.createdFrom) {
    params["Filter.CreatedFrom"] = filter.createdFrom;
  }

  if (filter.sort?.sortBy != null) {
    params["Filter.Sort.SortBy"] = filter.sort.sortBy;
  }

  if (filter.sort?.direction != null) {
    params["Filter.Sort.Direction"] = filter.sort.direction;
  }

  return params;
}

export const postService = {
  getPosts(options = {}) {
    const builtParams = buildPostQueryParams(options);
    console.log("GET /posts params:", builtParams);
    return apiClient.get("/posts", {
      params: builtParams,
    });
  },

  getPostById(id) {
    return apiClient.get(`/posts/${id}`);
  },

  createPost(postData) {
    return apiClient.post("/posts", postData);
  },

  updatePost(id, postData) {
    return apiClient.put(`/posts/${id}`, postData);
  },

  publishPost(id) {
    return apiClient.post(`/posts/${id}/publish`);
  },

  deletePost(id) {
    return apiClient.delete(`/posts/${id}`);
  },

  getMyDrafts(authorId, options = {}) {
    return apiClient.get("/posts", {
      params: buildPostQueryParams({
        page: 1,
        pageSize: 20,
        currentUserId: authorId,
        filter: {
          status: POST_STATUS.Draft,
          authorId,
          sort: {
            sortBy: POST_SORT_BY.CreatedAt,
            direction: SORT_DIRECTION.Desc,
          },
          ...options.filter,
        },
        ...options,
      }),
    });
  },

  getMyPublished(authorId, options = {}) {
    return apiClient.get("/posts", {
      params: buildPostQueryParams({
        page: 1,
        pageSize: 20,
        currentUserId: authorId,
        filter: {
          status: POST_STATUS.Published,
          authorId,
          sort: {
            sortBy: POST_SORT_BY.PublishedAt,
            direction: SORT_DIRECTION.Desc,
          },
          ...options.filter,
        },
        ...options,
      }),
    });
  },
};
