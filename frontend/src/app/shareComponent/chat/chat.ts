import { ChangeDetectorRef, Component, OnDestroy, OnInit } from '@angular/core';
import { ChatMessage, ChatService } from '../../services/chatService';
import { Subscription } from 'rxjs';
import { TokenService } from '../../services/token-service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-chat',
  imports: [FormsModule, CommonModule],
  templateUrl: './chat.html',
  styleUrl: './chat.css',
})
export class Chat implements OnInit, OnDestroy {
  messages: ChatMessage[] = [];
  newMessage = '';

  private sub!: Subscription;

  constructor(
    private chatService: ChatService,
    public tokenService: TokenService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
  this.chatService.loadMessages();      // 1️⃣ load history first
  this.chatService.startConnection();   // 2️⃣ then real-time
  this.sub = this.chatService.messages$
    .subscribe(msgs => {
      this.messages = msgs;
      this.cdr.detectChanges(); 
    });
}


  send(): void {
  if (!this.newMessage.trim()) return;

  const message: ChatMessage = {
    senderId: this.tokenService.getUserId(),
    senderRole: this.tokenService.getRole() || '',
    senderName: this.tokenService.getRole() || 'User',
    message: this.newMessage,
  };

  this.chatService.sendMessage(message).subscribe();
  this.newMessage = '';
}


  ngOnDestroy(): void {
    this.sub?.unsubscribe();
    this.chatService.stopConnection();
  }
}