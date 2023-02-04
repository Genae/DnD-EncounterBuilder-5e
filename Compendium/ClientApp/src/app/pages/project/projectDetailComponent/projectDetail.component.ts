import { Component } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Project } from '../../../models/project';

import { ProjectService } from '../../../services/project.service';

@Component({
    selector: 'projectDetail',
    templateUrl: 'projectDetail.component.html'
})

export class ProjectDetailComponent {

    project: Project;

    constructor(private projectService: ProjectService, private route: ActivatedRoute, private router: Router) {
        this.route.params.subscribe(params => {
            if (params['id']) 
                this.projectService.getProjectById(params['id']).subscribe(response => this.projectUpdated(response));
        });
    }

    projectUpdated(project: Project): void {
        if (project) {
            this.project = project;
        }
    }

    goToProject(project: Project): void {
        this.router.navigateByUrl('/projectDetails/' + project.id);
    }
}

