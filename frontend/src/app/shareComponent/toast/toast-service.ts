import { Injectable, NgZone } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

export type ToastType = 'success' | 'error' | 'info' | 'warning';

export interface ToastMessage {
  id: number;
  message: string;
  type: ToastType;
}

@Injectable({
  providedIn: 'root',
})
export class ToastService {

  private toasts$ = new BehaviorSubject<ToastMessage[]>([]);
  private counter = 0;

  constructor(private zone: NgZone) {}

  getToastStream() {
    return this.toasts$.asObservable();
  }

  show(message: string, type: ToastType = 'info', duration = 3000) {

    const toast: ToastMessage = {
      id: ++this.counter,
      message,
      type,
    };

    // ✅ run inside Angular zone
    this.zone.run(() => {

      this.toasts$.next([...this.toasts$.value, toast]);

      setTimeout(() => {
        this.remove(toast.id);
      }, duration);

    });
  }

  success(message: string) {
    this.show(message, 'success', 3000);
  }

  error(message: string) {
    this.show(message, 'error', 3000);
  }

  warning(message: string) {
    this.show(message, 'warning', 3000);
  }

  info(message: string) {
    this.show(message, 'info', 3000);
  }

  remove(id: number) {
    this.zone.run(() => {
      this.toasts$.next(
        this.toasts$.value.filter(t => t.id !== id)
      );
    });
  }
}