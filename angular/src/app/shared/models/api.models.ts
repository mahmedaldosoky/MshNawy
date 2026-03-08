/**
 * Shared API response and pagination models
 * Used across all API calls to maintain consistency
 * Per Constitution I: All error messages must be in Arabic only
 */

/**
 * Generic API error response
 */
export interface ErrorResponse {
  code: string;
  message: string; // Arabic message
  details?: string;
  timestamp?: string;
}

/**
 * Generic API error response envelope
 */
export interface ApiErrorResponse {
  error: ErrorResponse;
  traceId?: string;
}

/**
 * Pagination metadata for list responses
 */
export interface PaginationMeta {
  page: number; // 1-indexed
  pageSize: number;
  totalCount: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}

/**
 * Legacy paged result - kept for backward compatibility
 */
export interface PagedResult<T> {
  items: T[];
  totalCount: number;
}

/**
 * Generic paginated list response
 * Used for all collection endpoints (replaces PagedResult)
 */
export interface PaginatedResponse<T> {
  items: T[];
  pagination: PaginationMeta;
}

/**
 * Simple success response envelope
 */
export interface ApiResponse<T> {
  success: boolean;
  data: T;
  message?: string; // Optional Arabic message
}

/**
 * Request parameters for pagination
 */
export interface PaginationRequest {
  page?: number; // Default: 1
  pageSize?: number; // Default: 10
}

/**
 * Request parameters for filtering and sorting
 */
export interface FilterRequest {
  searchText?: string;
  sortBy?: string;
  sortOrder?: 'asc' | 'desc'; // Default: 'asc'
}

/**
 * Combined query parameters for list endpoints
 */
export interface ListRequest extends PaginationRequest, FilterRequest {}

/**
 * Standard HTTP status codes used in API
 */
export enum HttpStatus {
  OK = 200,
  Created = 201,
  BadRequest = 400,
  Unauthorized = 401,
  Forbidden = 403,
  NotFound = 404,
  Conflict = 409,
  InternalServerError = 500,
  ServiceUnavailable = 503
}

/**
 * Standard entity with timestamps (from ABP FullAuditedEntity)
 */
export interface AuditedEntity {
  id: string;
  creationTime: string; // ISO 8601 UTC
  creatorId?: string;
  lastModificationTime?: string; // ISO 8601 UTC
  lastModifierId?: string;
}

/**
 * Idempotency key for financial operations
 * Per Constitution III: All financial operations must be idempotent
 */
export interface IdempotentRequest {
  idempotencyKey: string; // UUID v4
}
