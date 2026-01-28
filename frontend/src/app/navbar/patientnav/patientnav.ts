import { Component } from '@angular/core';
import { TokenService } from '../../services/token-service';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';

@Component({
  selector: 'app-patientnav',
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './patientnav.html',
  styleUrl: './patientnav.css',
})
export class Patientnav {
  constructor(private router: Router, private tokenService: TokenService) { }
  logout() {
    this.tokenService.removeToken();
    this.router.navigate(['/patient-login']);
  }
}