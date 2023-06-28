import {AfterViewInit, ChangeDetectorRef, Component, Input, OnChanges, ViewChild} from '@angular/core';
import {BehaviorSubject, Observable} from "rxjs";
import {environment} from "@env/env";
import {arraysEqual} from "ng-zorro-antd/core/util";
import {ProductService} from "../../services/product.service";
import {PaymentsComponent} from "../payments/payments.component";
import {ModalButtonOptions, NzModalService} from "ng-zorro-antd/modal";
import {HeaderComponent} from "../header/header.component";
import {Router} from "@angular/router";

@Component({
  selector: 'app-cart',
  templateUrl: './cart.component.html',
  styleUrls: ['./cart.component.less']
})
export class CartComponent implements AfterViewInit{
  @ViewChild('payments') payments!: PaymentsComponent
  @ViewChild('header') header!: HeaderComponent
  constructor(
    protected productService: ProductService,
    protected modal: NzModalService,
    protected router: Router
  ) {
  }
  @Input() visible = false;
  @Input() product = new BehaviorSubject<any>([]);
  listProductCarts: any[] = [];
  productPayments: any[] = [];

  title = 'Giỏ hàng';
  total: number = 0;
  unit = environment.unitMoney;
  setOfCheckedId = new Set<number>();
  checked = false;
  indeterminate = false;

  closeDrawer() {
    this.visible = !this.visible;
  }
  openCart(data? : any) {
    if (data) {
      this.productService.addProductToCart(data.id, environment.language, data.quantity).toPromise().then((res) => {
        this.getOrderFromCart();
      })
    }
    else this.getOrderFromCart();
  }

  ngAfterViewInit(): void {
  }
  async getOrderFromCart(): Promise<void> {
    await this.productService.getOrder().toPromise().then((res: any) => {
      if (res) {
        this.listProductCarts = res;
        this.productPayments = res;
        this.getTotal();
      }
    })
      .finally(() => {
        this.visible = true;
      });
  }

  getTotal(){
    const total = 0;
    const totalMoney = this.productPayments.reduce((accumulator, currentValue) =>
      accumulator + (currentValue.price * currentValue.quantity), total
    )
    this.total = totalMoney;
  }

  onItemChecked(id: number,e: boolean): void {
    if (e){
      this.productPayments.push(this.listProductCarts.find(item => item.productId === id));
    }
    else {
      this.productPayments = this.listProductCarts.filter((item: any) => {
        return item.productId !== id;
      })
    }
      this.getTotal();
  }

  changeQuantiny(data: any, isRemoveProduct?: boolean, e?: any) {
    const quantiny = (isRemoveProduct || e === 0) ? 0 : e;
    this.productService.patchOrder(data.productId, quantiny).toPromise().then((res: any) => {
      if (res) {
        this.listProductCarts = res;
        this.productPayments = res;
        this.getTotal();
      }
    })
  }

  payment() {
    const isLogin = localStorage.getItem('name') ? true : false
    this.closeDrawer();
    if (!isLogin) {
      const productPaymentsJson = JSON.stringify(this.productPayments);
      this.modal.warning({
        nzTitle: 'Chú ý!',
        nzContent: 'Đăng nhập tài khoản người dùng để thanh toán',
        nzOnOk: () => this.router.navigate(['/account',2, productPaymentsJson, this.total])
      })
    }else this.payments.openDrawer(isLogin)
  }
}

