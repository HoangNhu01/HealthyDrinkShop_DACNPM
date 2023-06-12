import {ChangeDetectorRef, Component, Input, OnInit, ViewChild} from '@angular/core';
import {Router, Routes} from "@angular/router";
import {CartComponent} from "../cart/cart.component";
import {FormBuilder, FormGroup, Validators} from "@angular/forms";
import {environment} from "@env/env";
import {ProductService} from "../../services/product.service";
import {NzModalService} from "ng-zorro-antd/modal";
import {NzMessageService} from "ng-zorro-antd/message";
import {Guid} from "guid-typescript";

@Component({
  selector: 'app-payments',
  templateUrl: './payments.component.html',
  styleUrls: ['./payments.component.less']
})
export class PaymentsComponent implements OnInit{
  constructor(
    protected router: Router,
    protected cdr: ChangeDetectorRef,
    protected fb: FormBuilder,
    protected productService: ProductService,
    protected modalService: NzModalService,
    protected message: NzMessageService
  ) {
    this.initForm();
  }
  visible = false;
  form!: FormGroup;
  listProvince: any[] = [];
  listDistrict: any[] = [];
  listWard: any[] = [];
  @Input() productPayment: any[] = [];
  @Input() totalMoney: number = 0
  unit= environment.unitMoney
  ngOnInit(): void {
    this.productService.getProvinces().toPromise().then(((res: any) => {
      if (res) {
        this.listProvince = res;
      }
    }))
  }
  initForm() {
    this.form = this.fb.group({
      email: [localStorage.getItem('email') ? localStorage.getItem('email') : '',
      Validators.email],
      name: [localStorage.getItem('name') ? localStorage.getItem('name') : '',
      Validators.required],
      sdt: ['', [Validators.required, Validators.pattern('(84|0[3|5|7|8|9])+([0-9]{8})\\b')]],
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

    function getCurrentTime() {
      const today = new Date();
      const date = today.getFullYear()+'-'+(today.getMonth()+1)+'-'+today.getDate();
      const time = today.getHours() + ":" + today.getMinutes() + ":" + today.getSeconds();
      const dateTime = date+' '+time;
      return dateTime
    }

    if(this.form.valid) {
      const body = {
        orderId: Guid.create()["value"],
        userId: localStorage.getItem('userId'),
        userName: this.form.value.name,
        phoneNumber: this.form.value.sdt,
        address: this.form.value.address + ' ' +
          this.form.value.ward + ' ' +
        this.form.value.district + ' ' +
          this.form.value.province,
        email: this.form.value.email,
        paymentStatus: this.form.value.payments === 'normal' ? 0 : 1,
        totalPrice: this.totalMoney,
        cartItems: this.productPayment,
        orderDate: getCurrentTime()
      }
      if (this.form.value.payments === 'eWallet')
      {
          this.productService.eWalletPayments(body).toPromise().then((res: any) => {
            if (res) {
              window.open(res["data"]);
            }
          })
      }
      else {
        this.submitPayments(body);
      }
    }
  }

  getSum(num1: number, num2: number) {
    return Number(num1) + Number(num2);
  }

  changeProvince(e: any) {
    this.form.get('district')?.reset();
    this.form.get('ward')?.reset();
    this.listDistrict = this.listProvince.find(item => {
      return item.name === e
    }).districts;
  }

  changeDistrict(e: any) {
    this.form.get('ward')?.reset();
    this.listWard = this.listDistrict.find(item => {
      return item.name === e
    })?.wards;
  }

  showConfirm(): void {
    this.modalService.confirm({
      nzTitle: 'Confirm',
      nzContent: 'Bạn chắc chắn muốn thanh toán?',
      nzOkText: 'Thanh toán',
      nzCancelText: 'Hủy bỏ',
      nzOnOk: () => {
        this.payment();
      }
    });
  }

  async submitPayments(body: object) {
    await this.productService.payments(body).toPromise().then(() => {
      this.message.success('Thanh toán thành công!')
    })
      .finally(() => {
        this.close();
        this.router.navigate(['/']);
      })
      .catch(() => {
        this.message.error('Thanh toán thất bại, vui lòng kiểm tra lại thông tin!')
      })
  }
}
