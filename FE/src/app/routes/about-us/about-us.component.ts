import {Component, OnInit} from '@angular/core';

@Component({
  selector: 'app-about-us',
  templateUrl: './about-us.component.html',
  styleUrls: ['./about-us.component.less']
})
export class AboutUsComponent{
  lat = 51.678418;
  lng = 7.809007;
  avaHoang: string = './assets/icon/59305757_2215989045398573_1271159978580770816_n.jpg';
  avaThanh: string = './assets/icon/23905400_391031978013197_3611495744112576409_n.jpg';
  avaTan: string = './assets/icon/255607942_1364196323996341_1643076643530674597_n.jpg';
}
