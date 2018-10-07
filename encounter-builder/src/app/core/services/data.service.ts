import { Injectable } from '@angular/core';
import { Http, Response } from '@angular/http';
import { Observable } from 'rxjs';

@Injectable()
export class DataService {
    
    constructor(private http: Http) {
    
    }


    getMonsters(): Observable<any[]> {
        return this.http.get(`api/monsters`)
            .map((res: Response) => res.json())
            .catch((error: any) => Observable.throw(error.json().error || 'Server error'));
    }
    getSpells(): Observable<any[]> {
        return this.http.get(`api/spells`)
            .map((res: Response) => res.json())
            .catch((error: any) => Observable.throw(error.json().error || 'Server error'));
    }
    getSpellsFromIds(ids: string[]): Observable<any[]> {
        return this.http.post(`api/spells`, ids)
            .map((res: Response) => res.json())
            .catch((error: any) => Observable.throw(error.json().error || 'Server error'));
    }
}