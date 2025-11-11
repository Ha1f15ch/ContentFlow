// Статус поста (enum)
export const PostStatus = {
  Draft: 0,
  Pending: 1,
  Published: 2,
  Archived: 3,
  Deleted: 4,
};

// Команда создания поста
export const CreatePostCommand = {
  title: '',
  content: '',
  authorId: 0,
  categoryId: 0,
};

// DTO поста (ответ от API)
export const PostDto = {
  id: 0,
  title: '',
  slug: '',
  excerpt: null,
  authorId: 0,
  authorName: '',
  authorAvatar: null,
  status: 0,
  createdAt: '',
  publishedAt: null,
  tags: [],
  commentCount: 0,
};

// Результат с пагинации
export const PostDtoPaginatedResult = {
  items: [],
  page: 1,
  pageSize: 10,
  totalCount: 0,
  totalPages: 1,
};