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
  currentUserId = null,
} = {}) {
  const params = {
    page,
    pageSize,
  };

  if (currentUserId != null) {
    params.currentUserId = currentUserId;
  }

  if (filter.search) {
    params["filter.search"] = filter.search;
  }

  if (filter.categoryId != null) {
    params["filter.categoryId"] = filter.categoryId;
  }

  if (filter.status != null) {
    params["filter.status"] = filter.status;
  }

  if (filter.authorId != null) {
    params["filter.authorId"] = filter.authorId;
  }

  if (filter.createdFrom) {
    params["filter.createdFrom"] = filter.createdFrom;
  }

  if (filter.sort?.sortBy != null) {
    params["filter.sort.sortBy"] = filter.sort.sortBy;
  }

  if (filter.sort?.direction != null) {
    params["filter.sort.direction"] = filter.sort.direction;
  }

  return params;
}

export const postService = {
  getPosts(options = {}) {
    return apiClient.get("/posts", {
      params: buildPostQueryParams(options),
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
