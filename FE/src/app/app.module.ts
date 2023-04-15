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
import {HeaderComponent} from "./header/header.component";
import {BestProductComponent} from "./best-product/best-product.component";
import {SpecialProductComponent} from "./special-product/special-product.component";
import {FooterComponent} from "./footer/footer.component";
import {TestimonialsComponent} from "./testimonials/testimonials.component";
import {ImageProductComponent} from "./image-product/image-product.component";
import {HomeComponent} from "./home/home.component";
import {DetailProductComponent} from "./detail-product/detail-product.component";
import {AllProductComponent} from "./all-product/all-product.component";

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
    AllProductComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    FormsModule,
    HttpClientModule,
    BrowserAnimationsModule,
    IconsProviderModule,
    NzLayoutModule,
    NzMenuModule,
    NzButtonModule,
    NzDropDownModule,
  ],
  providers: [
    { provide: NZ_I18N, useValue: en_US }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
