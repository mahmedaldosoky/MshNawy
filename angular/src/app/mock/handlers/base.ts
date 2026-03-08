import { rest } from 'msw'

export const baseHandlers = [
  // Example: health
  rest.get('/api/health', (req, res, ctx) => {
    return res(ctx.status(200), ctx.json({ status: 'ok' }))
  })
]
