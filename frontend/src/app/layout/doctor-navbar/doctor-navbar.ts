import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Doctornav } from '../../navbar/doctornav/doctornav';
import { Footer } from '../footer/footer';

@Component({
  selector: 'app-doctor-navbar',
  imports: [RouterOutlet, Doctornav, Footer],
  templateUrl: './doctor-navbar.html',
  styleUrl: './doctor-navbar.css',
})
export class DoctorNavbar {

}
