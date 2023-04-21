import {AfterViewInit, Component, Input} from '@angular/core';
import { IsLoadingService } from '@service-work/is-loading';
import {Observable} from "rxjs";

@Component({
  selector: 'app-account',
  templateUrl: './account.component.html',
  styleUrls: ['./account.component.less']
})
export class AccountComponent {
  isLoading = false
  constructor(
  ) {}

  isLogin : any = true;

  getSignIn() {
    this.isLogin = !this.isLogin;
  }
}
