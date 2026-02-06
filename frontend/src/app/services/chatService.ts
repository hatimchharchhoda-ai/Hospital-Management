import { HttpClient } from '@angular/common/http';
import { Injectable, NgZone } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { BehaviorSubject } from 'rxjs';
import { TokenService } from './token-service';

export interface ChatMessage {
  id?: number;
  senderId: number;
  senderRole: string;
  senderName: string;
  message: string;
  sentAt?: string;
}

@Injectable({
  providedIn: 'root',
})
export class ChatService {
  private hubConnection!: signalR.HubConnection;

  constructor(
    private http: HttpClient,
    private tokenService: TokenService,
    private zone: NgZone
  ) { }

  private aUrl = `http://localhost:5001/api/Chat`;
  private hubUrl = `http://localhost:5001/chatHub`;

  private messagesSubject = new BehaviorSubject<ChatMessage[]>([]);
  messages$ = this.messagesSubject.asObservable();



  // ================= SIGNALR =================
  startConnection(): void {
  const token = this.tokenService.getToken();

  this.hubConnection = new signalR.HubConnectionBuilder()
    .withUrl(this.hubUrl, {
      accessTokenFactory: () => token || '',
    })
    .withAutomaticReconnect()
    .build();

  this.hubConnection.on('ReceiveMessage', (message: ChatMessage) => {
    this.zone.run(() => {
      this.messagesSubject.next([
        ...this.messagesSubject.value,
        message
      ]);
    });
  });

  this.hubConnection
    .start()
    .then(() => console.log('✅ SignalR connected'))
    .catch(err => console.error('❌ SignalR error:', err));
}


  stopConnection(): void {
    this.hubConnection?.stop();
  }

  // ================= API =================
  loadMessages(): void {
  this.http.get<ChatMessage[]>(`${this.aUrl}/messages`)
    .subscribe(messages => {
      this.zone.run(() => {
        this.messagesSubject.next(messages);
      });
    });
}


  sendMessage(message: ChatMessage) {
    return this.http.post(`${this.aUrl}/send`, message);
  }
}
