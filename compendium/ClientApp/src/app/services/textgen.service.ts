import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Project } from "../models/project";
import { Action, Monster, Multiattack } from '../models/monster';


@Injectable()
export class TextgenService {

    constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) { }

    generateMultiattackText(monster: Monster): Observable<Multiattack> {
        return this.http.post<Multiattack>(this.baseUrl + 'api/textgen/multiattack', monster);
    }

    generateActionText(action: Action): Observable<string> {
        return this.http.post<string>(this.baseUrl + 'api/textgen/action', action);
    }
}
