import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Patientnav } from '../../navbar/patientnav/patientnav';
import { Footer } from '../footer/footer';

@Component({
  selector: 'app-patient-navbar',
  imports: [RouterOutlet, Patientnav, Footer],
  templateUrl: './patient-navbar.html',
  styleUrl: './patient-navbar.css',
})
export class PatientNavbar { }
