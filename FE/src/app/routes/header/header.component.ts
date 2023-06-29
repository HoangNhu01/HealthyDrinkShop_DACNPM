import {ChangeDetectorRef, Component, OnInit, ViewChild} from '@angular/core';
import {CartComponent} from "../cart/cart.component";
import {NzMessageService} from "ng-zorro-antd/message";
import {NzModalService} from "ng-zorro-antd/modal";
import {Router} from "@angular/router";

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.less']
})
export class HeaderComponent implements OnInit{
  constructor(
    protected modal: NzModalService,
    protected router: Router,
    protected message: NzMessageService
  ) {
  }
  name: any = ''
  textSearch = ''
  @ViewChild('cart') cartComp! : CartComponent;
  numberProductInCart = 0;

  openCart() {
    this.cartComp.openCart();
  }

  checkAcc(): boolean {
    if (localStorage.getItem('name')) {
      this.name = localStorage.getItem('name');
      return true
    }
    else return false
  }

  logout() {
    this.modal.warning({
      nzTitle: 'Chú ý!',
      nzContent: 'Bạn có chắc chắn muốn đăng xuất khỏi hệ thốnng',
      nzOnOk: () => {
        this.removeAccount()
        this.message.success('Đăng xuất thành công')
        location.reload();
      }
    })
  }
  removeAccount() {
    localStorage.removeItem('name');
    localStorage.removeItem('email');
    localStorage.setItem('isRememberMe', 'false');
    localStorage.removeItem('userId')
  }

  ngOnInit(): void {
  }

  openListOrder() {
    const isLogin = localStorage.getItem('userId')
    if (!isLogin) {
      this.modal.warning({
        nzTitle: 'Chú ý!',
        nzContent: 'Đăng nhập tài khoản người dùng để xem đơn hàng',
        nzOnOk: () => this.router.navigate(['/account', 1] )
      })
    }
    else this.router.navigate(['/order-list'])
  }

  search() {
    this.router.navigate(['/all-product', this.textSearch])
  }
}
