import AppMainPage from "./appMainPage";
import NotFoundPage from "./NotFoundPage";
import JobInfoPage from "./jobInfoPage";

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