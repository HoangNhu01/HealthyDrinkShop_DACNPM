import {AfterViewInit, ChangeDetectorRef, Component, OnInit} from '@angular/core';
import { environment } from '@env/env'
import {HeaderComponent} from "./routes/header/header.component";
import {JwtHelperService} from "@auth0/angular-jwt";
import {NzMessageService} from "ng-zorro-antd/message";
import {ProductService} from "./services/product.service";
import {BehaviorSubject} from "rxjs";
import {Router} from "@angular/router";
import {NzModalService} from "ng-zorro-antd/modal";

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.less']
})
export class AppComponent extends HeaderComponent implements OnInit{
  constructor(
    protected override modal: NzModalService,
    protected override router: Router,
    protected override message: NzMessageService,
    public jwtHelper: JwtHelperService,
    ) {
    super(modal, router, message);
  }

  override ngOnInit(): void {
    if (localStorage.getItem('isRememberMe') == 'false' || this.isAuthenticated()) {
      this.removeAccount();
    }
  }
  isAuthenticated(): boolean {
    const token = sessionStorage.getItem('token');
    return !this.jwtHelper.isTokenExpired(token);
  }
}
