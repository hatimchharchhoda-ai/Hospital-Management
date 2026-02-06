import { ApplicationConfig, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { provideClientHydration, withEventReplay } from '@angular/platform-browser';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideAnimations } from '@angular/platform-browser/animations';
import { provideToastr } from 'ngx-toastr';
import { jwtInterceptor } from './interceptors/jwt.interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes), provideClientHydration(withEventReplay()),
    
    // ✅ Register JWT interceptor
    provideHttpClient(
      withInterceptors([jwtInterceptor])
    ),
    provideAnimations(),
    provideToastr(),

     // You can provide your backend API URL as an injectable token
    { provide: 'API_URL', useValue: 'http://localhost:5001/api' }
  ],
};
