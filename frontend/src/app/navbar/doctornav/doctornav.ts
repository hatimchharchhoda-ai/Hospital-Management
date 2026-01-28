import { Component } from '@angular/core';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { TokenService } from '../../services/token-service';

@Component({
  selector: 'app-doctornav',
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './doctornav.html',
  styleUrl: './doctornav.css',
})
export class Doctornav {
  constructor(
    private tokenService: TokenService,
    private router: Router
  ) {}

  logout() {
    this.tokenService.removeToken();
    this.router.navigate(['/doctor-login']);
  }
}
