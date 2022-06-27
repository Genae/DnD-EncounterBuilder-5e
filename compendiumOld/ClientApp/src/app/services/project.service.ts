import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Project } from "../models/project";


@Injectable()
export class ProjectService {

    constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) { }

    getProjectById(id: string): Observable<Project> {
        return this.http.get<Project>(this.baseUrl + 'api/project/' + id);
    }

    getProjects(): Observable<Project[]> {
        return this.http.get<Project[]>(this.baseUrl + 'api/project');
    }

    createNewProject(project: Project): Observable<Project> {
        return this.http.post<Project>(this.baseUrl + 'api/project', project);
    }

    updateProject(project: Project): Observable<Project> {
        return this.http.post<Project>(this.baseUrl + 'api/project/' + project.id, project);
    }

    deleteProject(project: Project): Observable<Project> {
        return this.http.delete<Project>(this.baseUrl + 'api/project/' + project.id);
    }
}
