import {Component, OnInit} from '@angular/core';
import {ProductService} from "../../services/product.service";
import {NzModalService} from "ng-zorro-antd/modal";
import {NzMessageService} from "ng-zorro-antd/message";

@Component({
  selector: 'app-order-list',
  templateUrl: './order-list.component.html',
  styleUrls: ['./order-list.component.less']
})
export class OrderListComponent implements OnInit{
  constructor(
    protected orderService: ProductService,
    protected modal: NzModalService,
    protected message: NzMessageService
  ) {
  }
  index = 0;
  listOrder: any[] = [];
  listOrderByStatus: any[] = [];
  ngOnInit() {
    this.getListOrder();
  }
  getListOrder() {
    const userId = localStorage.getItem('userId');
    if (userId) {
      this.orderService.getListOrderByUserId(userId).toPromise().then((res: any) => {
        this.listOrder = res.resultObj;
          this.listOrderByStatus = this.listOrder.filter((item: any) => {
            return item.paymentStatus === this.index
          })
      })
    }
  }

  onIndexChange(e: number) {
    this.index = e;
    this.getListOrder();
  }

  cancelOrder(id: string) {
    if (id) {
      this.modal.warning({
        nzTitle: 'Chú ý!',
        nzContent: 'Bạn có chắc chắn muốn hủy đơn hàng',
        nzOnOk: () => {
          this.orderService.cancelOrderById(id).toPromise().then((res: any) => {
            if (res) {
              this.listOrderByStatus = this.listOrderByStatus.filter((item: any) => {
                return item.id != id
              })
              this.message.success('Hủy đơn thành công')
            }
          })
        }
      })
    }
  }
}
