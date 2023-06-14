import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import {BehaviorSubject} from "rxjs";
@Injectable({
  providedIn: 'root'
})
export class SignalRServiceService {
  receiveData = new BehaviorSubject<any>('');
  private hubConnection!: signalR.HubConnection;
  constructor() {}
  getSignalRData() {
    this.hubConnection = new signalR.HubConnectionBuilder().withUrl("https://localhost:5002/orderHub", {
      skipNegotiation: true,
      transport: signalR.HttpTransportType.WebSockets
    }).build();

    this.hubConnection.on('StatusOrderMessage', (data: any) => {
      console.log(data);
      // Handle the received message data
      this.receiveData.next(data);
    });
    this.hubConnection.start()
      .then(() => console.log('SignalR connection started'))
      .catch(err => console.log('Error while starting SignalR connection: ', err));
  }
}
