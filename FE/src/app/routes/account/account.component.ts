import {AfterViewInit, Component, Input} from '@angular/core';
import { IsLoadingService } from '@service-work/is-loading';
import {Observable} from "rxjs";
import {FormBuilder, FormGroup, Validators} from "@angular/forms";
import {ProductService} from "../../services/product.service";
import {Router} from "@angular/router";
import {NzModalService} from "ng-zorro-antd/modal";
import {CookieService} from 'ngx-cookie-service';
import {NzMessageService} from "ng-zorro-antd/message";
@Component({
  selector: 'app-account',
  templateUrl: './account.component.html',
  styleUrls: ['./account.component.less']
})
export class AccountComponent {
  constructor(
    protected fb: FormBuilder,
    protected authService: ProductService,
    protected router: Router,
    private cookieService: CookieService,
    private message: NzMessageService
  ) {
    this.initForm();
  }
  isLoading = false

  isLogin : any = true;
  loginForm!: FormGroup;
  rememberMe = true;

  initForm(): void {
    this.loginForm = this.fb.group({
      username: ['', Validators.required],
      password: ['', Validators.required],
    })
    }

  getSignIn() {
    this.isLogin = !this.isLogin;
  }

  login() {
    this.isLoading = true;
    if (this.loginForm.invalid){
      this.loginForm.markAsDirty();
    }
    else {
      const body = {
        username: this.loginForm.value.username,
        password: this.loginForm.value.password,
        rememberMe: this.rememberMe
      }
      this.authService.login(body).toPromise().then((res: any) => {
        if(res && res.isSuccessed) {
          this.saveInfoToLocalStorage(res.resultObj)
          this.router.navigate(['/']);
          this.isLoading = false;
          this.message.create('success', 'Đăng nhập thành công');
        }
      } )
    }
  }
  getUserInfo(token: string) : any {
      const base64Url = token.split('.')[1];
      const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
      const jsonPayload = decodeURIComponent(
        atob(base64)
          .split('')
          .map(function(c) {
            return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
          })
          .join('')
      );

      return JSON.parse(jsonPayload);
  }
  saveInfoToLocalStorage(token: string) {
    this.cookieService.set('token', token);
    localStorage.setItem('isRememberMe', this.rememberMe.toString());
    const userInfo = this.getUserInfo(token);
    localStorage.setItem('name', userInfo["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"]);
    localStorage.setItem('email', userInfo["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"]);
  }
}
