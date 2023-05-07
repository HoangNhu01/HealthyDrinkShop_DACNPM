import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { NZ_I18N } from 'ng-zorro-antd/i18n';
import { en_US } from 'ng-zorro-antd/i18n';
import { registerLocaleData } from '@angular/common';
import en from '@angular/common/locales/en';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { IconsProviderModule } from './icons-provider.module';
import { NzLayoutModule } from 'ng-zorro-antd/layout';
import { NzMenuModule } from 'ng-zorro-antd/menu';
import {NzButtonModule} from "ng-zorro-antd/button";
import {NzDropDownModule} from "ng-zorro-antd/dropdown";
import {HeaderComponent} from "./routes/header/header.component";
import {BestProductComponent} from "./routes/best-product/best-product.component";
import {SpecialProductComponent} from "./routes/special-product/special-product.component";
import {FooterComponent} from "./routes/footer/footer.component";
import {TestimonialsComponent} from "./routes/testimonials/testimonials.component";
import {ImageProductComponent} from "./routes/image-product/image-product.component";
import {HomeComponent} from "./routes/home/home.component";
import {DetailProductComponent} from "./routes/detail-product/detail-product.component";
import {AllProductComponent} from "./routes/all-product/all-product.component";
import {AccountComponent} from "./routes/account/account.component";
import {NzSpinModule} from "ng-zorro-antd/spin";
import {CartComponent} from "./routes/cart/cart.component";
import {NzDrawerModule} from "ng-zorro-antd/drawer";

registerLocaleData(en);

@NgModule({
  declarations: [
    AppComponent,
    HeaderComponent,
    BestProductComponent,
    SpecialProductComponent,
    FooterComponent,
    TestimonialsComponent,
    ImageProductComponent,
    HomeComponent,
    DetailProductComponent,
    AllProductComponent,
    AccountComponent,
    CartComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    AppRoutingModule,
    FormsModule,
    BrowserAnimationsModule,
    IconsProviderModule,
    NzLayoutModule,
    NzMenuModule,
    NzButtonModule,
    NzDropDownModule,
    NzSpinModule,
    NzDrawerModule,
  ],
  providers: [
    { provide: NZ_I18N, useValue: en_US }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
