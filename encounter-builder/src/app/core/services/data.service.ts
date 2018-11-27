
import {throwError as observableThrowError,  Observable } from 'rxjs';

import {map, catchError} from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { Http, Response } from '@angular/http';


@Injectable()
export class DataService {
    
    constructor(private http: Http) {
    
    }

    getMonsterById(id: string): Observable<any> {
        return this.http.get(`api/monsters/` + id).pipe(
            map((res: Response) => res.json()),
            catchError((error: any) => observableThrowError(error.json().error || 'Server error')));
    }

    getMonsters(): Observable<any[]> {
        return this.http.get(`api/monsters`).pipe(
            map((res: Response) => res.json()),
            catchError((error: any) => observableThrowError(error.json().error || 'Server error')),);
    }
    getSpells(): Observable<any[]> {
        return this.http.get(`api/spells`).pipe(
            map((res: Response) => res.json()),
            catchError((error: any) => observableThrowError(error.json().error || 'Server error')),);
    }
    getSpellsFromIds(ids: string[]): Observable<any[]> {
        return this.http.post(`api/spells`, ids).pipe(
            map((res: Response) => res.json()),
            catchError((error: any) => observableThrowError(error.json().error || 'Server error')),);
    }
}