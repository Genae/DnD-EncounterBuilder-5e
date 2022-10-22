import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { WeaponType } from '../models/weapon';


@Injectable()
export class WeaponTypeService {

    constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) { }


    getWeapons(): Observable<WeaponType[]> {
        return this.http.get<WeaponType[]>(this.baseUrl + 'api/weaponType');
    }
}
