import {AfterViewInit, Component, Input} from '@angular/core';
import { IsLoadingService } from '@service-work/is-loading';
import {Observable} from "rxjs";
import {AbstractControl, FormBuilder, FormGroup, Validators} from "@angular/forms";
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
  registerForm!: FormGroup;
  isShowPassLogin = false;
  isShowPassRegistry = false;
  isShowRePassRegistry = false;
  patternPassword = '^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$'
  patternEmail = '"^[a-z0-9._%+-]+@[a-z0-9.-]+\\\\.[a-z]{2,4}$"'

  initForm(): void {
    this.loginForm = this.fb.group({
      username: ['', Validators.required],
      password: ['', Validators.required],
    })
    this.registerForm = this.fb.group({
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      dob: ['', Validators.required],
      numberPhone: ['', [Validators.required, Validators.pattern('(84|0[3|5|7|8|9])+([0-9]{8})\\b')]],
      email: ['', Validators.required, Validators.pattern(this.patternEmail)],
      username: ['', Validators.required],
      password: ['', [Validators.required, Validators.pattern(this.patternPassword)]],
      rePassword: ['', Validators.required],
    },
      {
        validator: comparePassword
      })
    }

  getSignIn() {
    this.isLogin = !this.isLogin;
    this.isShowPassLogin = false
    this.isShowPassRegistry = false
    this.isShowRePassRegistry = false
    this.loginForm.reset()
    this.registerForm.reset()
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
        .catch(() => {
          this.isLoading = false;
          this.message.create('error', 'Đăng nhập thất bại');
        })
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
    localStorage.setItem('userId', userInfo["UserId"]);
    localStorage.setItem('name', userInfo["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"]);
    localStorage.setItem('email', userInfo["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"]);
  }

  signUp() {
    if (this.checkValidateFormRegistry()) {
      const body = {
        firstName: this.registerForm.value.firstName,
        lastName: this.registerForm.value.lastName,
        dob: this.registerForm.value.dob,
        email: this.registerForm.value.email,
        phoneNumber: this.registerForm.value.numberPhone,
        userName: this.registerForm.value.username,
        password: this.registerForm.value.password,
        confirmPassword: this.registerForm.value.rePassword,
      }
      this.authService.registry(body).toPromise().then((res: any) => {
        if (res && res.isSuccessed) {
          this.message.success('Đăng kí tài khoản thành công!')
          this.getSignIn();
        }
        else this.message.error(res.message);
      })
    }
  }
  checkValidateFormRegistry(): boolean {
    this.registerForm.markAllAsTouched();
    if (this.registerForm.valid) return false
    else return true
  }
}
export function comparePassword(c: AbstractControl) {
  const v = c.value;
  return (v.password === v.rePassword) ? null : {
    passwordnotmatch: true
  };
}
