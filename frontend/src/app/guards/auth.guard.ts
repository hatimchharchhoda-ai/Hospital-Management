import { ActivatedRouteSnapshot, CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { TokenService } from '../services/token-service';

export const authGuard: CanActivateFn = (route) => {
  const tokenService = inject(TokenService);
  const router = inject(Router);

  // 1️⃣ Check login
  if (!tokenService.isLoggedIn()) {
    router.navigate(['/patient-login']);
    return false;
  }

  const expectedRole = route.data?.['role'];
  const userRole = tokenService.getUserRole();

  if (expectedRole && userRole !== expectedRole) {
    router.navigate(['/unauthorized']);
    return false;
  }

  return true;
};