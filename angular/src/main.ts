import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';
import { AppModule } from './app/app.module';

// Per Constitution VI: Frontend-first delivery requires functional mock API
// MSW is started conditionally based on environment (see environments/environment.ts)
async function bootstrap() {
  // Only start MSW if mocks are enabled in the current environment
  const env = await import('./environments/environment');

  if (env.environment.mockApiEnabled) {
    const { worker } = await import('./app/mock/browser');

    try {
      await worker.start({
        onUnhandledRequest: 'bypass', // Allow unhandled requests to bypass MSW
        quiet: true // Suppress MSW startup logs in console
      });
      console.log('✓ Mock Service Worker started (frontend-first development)');
    } catch (error) {
      console.error('✗ MSW startup failed:', error);
    }
  }

  // Now bootstrap the Angular application
  platformBrowserDynamic()
    .bootstrapModule(AppModule)
    .catch(err => console.error('Bootstrap error:', err));
}

bootstrap();
