import { Component } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Project } from '../../../models/project';

import { ProjectService } from '../../../services/project.service';

@Component({
    selector: 'projectEdit',
    templateUrl: 'projectEdit.component.html'
})

export class ProjectEditComponent {

    project: Project;

    constructor(private projectService: ProjectService, private route: ActivatedRoute, private router: Router) {
        this.route.params.subscribe(params => {
            if (params['id'] && params['id'] != '0') 
                this.projectService.getProjectById(params['id']).subscribe(response => this.projectUpdated(response));
            if (params['id'] == '0')
                this.project = new Project();
        });
    }

    projectUpdated(project: Project): void {
        if (project) {
            this.project = project;
        }
    }

    onSubmit() {
        this.projectService.updateProject(this.project).subscribe(response => this.goToProject(response));
    }

    goToProject(project: Project): void {
        this.router.navigateByUrl('/projectDetail/' + project.id);
    }
}

