// MSW browser setup for frontend-first development
// Per Constitution VI: Mock API layer enables complete UI validation before backend
import { setupWorker } from 'msw';
import { baseHandlers } from './handlers/base';

// Initialize MSW with all available handlers
// Handlers are organized by domain (identity, wallet, offerings, etc.)
export const worker = setupWorker(...baseHandlers);

// MSW is started conditionally in main.ts based on environment.mockApiEnabled
