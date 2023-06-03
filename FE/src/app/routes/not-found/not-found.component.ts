import { Component } from '@angular/core';
import {NzMessageService} from "ng-zorro-antd/message";

@Component({
  selector: 'app-not-found',
  templateUrl: './not-found.component.html',
  styleUrls: ['./not-found.component.less']
})
export class NotFoundComponent {
  constructor(
    protected messeage: NzMessageService
  ) {
    this.messeage.error('Không tìm thấy nội dung!',)
  }

}
