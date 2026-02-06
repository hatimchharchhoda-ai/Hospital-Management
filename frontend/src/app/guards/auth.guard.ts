import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { TokenService } from '../services/token-service';

export const authGuard: CanActivateFn = (route, state) => {
  const tokenService = inject(TokenService);
  const router = inject(Router);

  const isLoggedIn = tokenService.isLoggedIn();
  const userRole = tokenService.getUserRole(); // Doctor | Patient
  const expectedRole =
    route.data?.['role'] ?? route.parent?.data?.['role'];

  const routePath = route.routeConfig?.path;

  console.log("user role: ",userRole);
  console.log("expected role:", expectedRole);
  
  /* ----------------------------------------
     1️⃣ AUTH ROUTES ONLY (exact match)
     ---------------------------------------- */
  const authRoutes = [
    'patient-login',
    'doctor-login',
    '',
    'doctor-signup'
  ];

  if (routePath && authRoutes.includes(routePath)) {
    if (isLoggedIn) {
      router.navigate([
        userRole === 'Doctor' ? '/doctor/home' : '/patient/home'
      ]);
      return false;
    }
    return true;
  }

  /* ----------------------------------------
     2️⃣ PROTECTED ROUTES
     ---------------------------------------- */
  if (!isLoggedIn) {
    router.navigate(['/patient-login']);
    return false;
  }

  /* ----------------------------------------
     3️⃣ ROLE CHECK
     ---------------------------------------- */
  if (expectedRole && userRole !== expectedRole) {
    router.navigate(['/unauthorized']);
    return false;
  }

  return true;
};