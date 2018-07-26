import { Injectable } from '@angular/core';
import { Http, Response } from '@angular/http';
import { Observable } from 'rxjs';

@Injectable()
export class DataService {
    
    constructor(private http: Http) {
    
    }


    getMonsters(): Observable<any[]> {
        return this.http.get(`api/default`)
            .map((res: Response) => res.json())
            .catch((error: any) => Observable.throw(error.json().error || 'Server error'));
    }
    
}