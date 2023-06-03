import {ChangeDetectorRef, Component, Input, ViewChild} from '@angular/core';
import {Router, Routes} from "@angular/router";
import {CartComponent} from "../cart/cart.component";
import {FormBuilder, FormGroup, Validators} from "@angular/forms";
import {environment} from "@env/env";

@Component({
  selector: 'app-payments',
  templateUrl: './payments.component.html',
  styleUrls: ['./payments.component.less']
})
export class PaymentsComponent {
  constructor(
    protected router: Router,
    protected cdr: ChangeDetectorRef,
    protected fb: FormBuilder
  ) {
    this.initForm();
  }
  visible = false;
  form!: FormGroup;
  @Input() productPayment: any[] = [];
  @Input() totalMoney: number = 0
  unit= environment.unitMoney
  initForm() {
    this.form = this.fb.group({
      email: [localStorage.getItem('email') ? localStorage.getItem('email') : '',
      Validators.email],
      name: [localStorage.getItem('name') ? localStorage.getItem('name') : '',
      Validators.required],
      sdt: ['', [Validators.required, Validators.pattern(' /(0[3|5|7|8|9])+([0-9]{8})\\b/g')]],
      province: ['', Validators.required],
      district: ['', Validators.required],
      ward: ['', Validators.required],
      address: ['', Validators.required],
      option: '',
      ship: [true],
      payments: ['', Validators.required]
    })
  }
  close() {
    this.visible = false;
  }
  openDrawer(data: any, isLogin: boolean){
    if(!isLogin) {
      this.router.navigate(['/account']);
    }
    else {
      this.visible = true;
    }
  }

  payment() {
    for (const i in this.form.controls) {
      this.form.controls[i].markAsDirty();
      this.form.controls[i].updateValueAndValidity();
    }
  }

  getSum(num1: number, num2: number) {
    return Number(num1) + Number(num2);
  }
}
