import { CommonModule } from '@angular/common';
import { AfterViewInit, ChangeDetectorRef, Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { Chart, ChartConfiguration, registerables } from 'chart.js';
Chart.register(...registerables);
import { DoctorService } from '../../services/doctor-service';
import { TokenService } from '../../services/token-service';
import { ToastService } from '../../shareComponent/toast/toast-service';

@Component({
  selector: 'app-dashboard',
  imports: [CommonModule],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css',
})
export class Dashboard implements OnInit, AfterViewInit {
  @ViewChild('statusChart') chartRef!: ElementRef<HTMLCanvasElement>;
  chart!: Chart;

  doctorId!: number;

  todayAppointments: any[] = [];
  upcomingAppointments: any[] = [];

  bookedCount = 0;
  cancelledCount = 0;
  completedCount = 0;

  constructor(private DoctorService: DoctorService,
    private tokenService: TokenService,
    private toast: ToastService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    const role = this.tokenService.getRole();
    const userId = this.tokenService.getUserId();

    if (role !== 'Doctor' || !userId) {
      this.toast.error('Unauthorized access');
      return;
    }

    this.doctorId = userId;

    this.loadTodayAppointments();
    this.loadUpcomingAppointments();
  }

  ngAfterViewInit(): void {
    this.createChart();
  }

  loadTodayAppointments() {
    this.DoctorService.getTodayAppointments(this.doctorId)
      .subscribe({
        next: (res) => {
          this.todayAppointments = res;
          // ✅ don't update chart here
          this.toast.success('Today appointments loaded');
          this.cdr.detectChanges();
        },
        error: () => {
          this.toast.error('Failed to load today appointments');
        }
      });
  }

  loadUpcomingAppointments() {
    this.DoctorService.getAllAppointmentsByDoctor(this.doctorId)
      .subscribe({
        next: (res) => {
          this.upcomingAppointments = res;

          // ✅ calculate status counts for the chart
          this.calculateStatus(res);

          // ✅ update chart
          this.updateChart();
          this.cdr.detectChanges();
        },
        error: () => {
          this.toast.warning('Could not load appointments');
        }
      });
  }

  calculateStatus(appointments: any[]) {
    this.bookedCount = appointments.filter(a => a.status === 1).length;
    this.cancelledCount = appointments.filter(a => a.status === 2).length;
    this.completedCount = appointments.filter(a => a.status === 3).length;
  }

  createChart() {
    const config: ChartConfiguration<'doughnut'> = {
      type: 'doughnut',
      data: {
        labels: ['Booked', 'Cancelled', 'Completed'],
        datasets: [{
          data: [0, 0, 0],
          backgroundColor: ['#0d6efd', '#dc3545', '#198754']
        }]
      },
      options: {
        responsive: true,
        plugins: {
          legend: { position: 'bottom' }
        }
      }
    };

    this.chart = new Chart(this.chartRef.nativeElement, config);
  }

  updateChart() {
    if (!this.chart) return;

    this.chart.data.datasets[0].data = [
      this.bookedCount,
      this.cancelledCount,
      this.completedCount
    ];
    this.chart.update();
  }

  getStatusLabel(status: number): string {
    return status === 1 ? 'Booked'
      : status === 2 ? 'Cancelled'
        : status === 3 ? 'Completed'
          : '';
  }
}
