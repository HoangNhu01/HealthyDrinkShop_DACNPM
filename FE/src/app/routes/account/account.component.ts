import {Component, OnInit, ViewChild} from '@angular/core';
import {AbstractControl, FormBuilder, FormGroup, Validators} from "@angular/forms";
import {ProductService} from "../../services/product.service";
import {ActivatedRoute, Router} from "@angular/router";
import {CookieService} from 'ngx-cookie-service';
import {NzMessageService} from "ng-zorro-antd/message";
import {Location} from '@angular/common';
import {PaymentsComponent} from "../payments/payments.component";

@Component({
  selector: 'app-account',
  templateUrl: './account.component.html',
  styleUrls: ['./account.component.less']
})
export class AccountComponent implements OnInit {
  @ViewChild('payments') payments!: PaymentsComponent
  logged = false;

  constructor(
    protected fb: FormBuilder,
    protected authService: ProductService,
    protected router: Router,
    private cookieService: CookieService,
    private message: NzMessageService,
    protected activatedRoute: ActivatedRoute,
    protected location: Location
  ) {
    this.initForm();
  }

  isLoading = false
  productPayment: any[] = [];
  total: number = 0

  isLogin: any = true;
  loginForm!: FormGroup;
  rememberMe = true;
  registerForm!: FormGroup;
  isShowPassLogin = false;
  isShowPassRegistry = false;
  isShowRePassRegistry = false;
  patternPassword = '^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$'
  patternEmail = '"^[a-z0-9._%+-]+@[a-z0-9.-]+\\\\.[a-z]{2,4}$"'
  directionalId = 0;

  ngOnInit() {
    this.logged = localStorage.getItem('userId') ? true : false
    this.activatedRoute.params.subscribe((params: any) => {
      this.directionalId = params['id']
      this.productPayment = params['product'] ? JSON.parse(params['product']) : [];
      this.total = params['total'] ? params['total'] : 0
    })
  }

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
        email: ['', [Validators.required]],
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
    if (this.loginForm.invalid) {
      this.loginForm.markAsDirty();
    } else {
      const body = {
        username: this.loginForm.value.username,
        password: this.loginForm.value.password,
        rememberMe: this.rememberMe
      }
      this.authService.login(body).toPromise().then((res: any) => {
        if (res && res.isSuccessed) {
          this.logged = true
          this.saveInfoToLocalStorage(res.resultObj)
          if (this.directionalId == 1) this.router.navigate(['/order-list']);
          else if (this.directionalId == 2) {
            this.payments.openDrawer(true)
          } else if (!this.directionalId) this.router.navigate(['/']);
          else this.location.back()
          this.message.create('success', 'Đăng nhập thành công');
        } else this.message.create('error', res.message);
      })
        .catch(() => {
          this.logged = false;
          this.message.create('error', 'Đăng nhập thất bại');
        })
        .finally(() => {
          this.isLoading = false;
        })
    }
  }

  getUserInfo(token: string): any {
    const base64Url = token.split('.')[1];
    const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
    const jsonPayload = decodeURIComponent(
      atob(base64)
        .split('')
        .map(function (c) {
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
      this.isLoading = true;
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
          this.message.success(res.resultObj);
          this.getSignIn();
        } else this.message.error(res.message);
      })
        .finally(() => {
          this.isLoading = false
        })
    }
  }

  checkValidateFormRegistry(): boolean {
    this.registerForm.markAllAsTouched();
    if (this.registerForm.valid) return true
    else return false
  }
}

export function comparePassword(c: AbstractControl) {
  const v = c.value;
  return (v.password === v.rePassword) ? null : {
    passwordnotmatch: true
  };
}
