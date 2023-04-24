import { HttpClient } from '@angular/common/http';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent {
  @Output() cancelRegister = new EventEmitter();
  model : any ={}

  constructor(private http: HttpClient, private accountService: AccountService)
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
        console.log(error);
      }
    });
  }

  cancel()
  {
    this.cancelRegister.emit(false);
    console.log('cancelled')
  }
}
