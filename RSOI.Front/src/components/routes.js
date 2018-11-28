import AppMainPage from "./appMainPage";
import NotFoundPage from "./NotFoundPage";
import JobInfoPage from "./jobInfoPage";
import CreateJobPage from "./createJobPage";
import UpdateJobPage from "./updateJobPage";
import PageShower from "./pagesShower";

export default [
    {
        path: '/',
        component: AppMainPage,
        options: {
            history: true,
            pushState: true
        }
    },
    {
        path: '/jobInfo/:jobId',
        component: JobInfoPage,
        options: {
            history: true,
            pushState: true
        }
    },
    {
        path: '/jobInfo/:jobId/pages',
        component: PageShower,
        options: {
            history: true,
            pushState: true
        }
    },
    {
        path: '/createJob',
        component: CreateJobPage,
        options: {
            history: true,
            pushState: true
        }
    },
    {
        path: '/updateJob',
        component: UpdateJobPage,
        options: {
            history: true,
            pushState: true
        }
    }
];

export const notFoundRoute = {
    path: '(.*)',
    component: NotFoundPage,
    options: {
        history: true,
        pushState: true
    }
};