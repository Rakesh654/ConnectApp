import { HttpClient } from '@angular/common/http';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent {
  @Output() cancelRegister = new EventEmitter();
  model : any ={}

  constructor(private http: HttpClient, private accountService: AccountService, private toaster : ToastrService)
  {

  }

  ngOnInit():void{

  } 

  register(model : any)
  {
    this.accountService.register(model).subscribe({
      next : () =>{
        this.cancel();
        
      },
      error : error =>
      {
        this.toaster.error(error.error);
      }
    });
  }

  cancel()
  {
    this.cancelRegister.emit(false);
    console.log('cancelled')
  }
}
