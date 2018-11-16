import React from "react";
import ReactDOM from 'react-dom';
import DevTools from "mobx-react-devtools";
import createBrowserHistory from 'history/createBrowserHistory';
import {RouterStore, syncHistoryWithStore } from "mobx-react-router";

// Import Framework7
import Framework7 from 'framework7/framework7.esm.bundle';

// Import Framework7-React plugin
import Framework7React from 'framework7-react';

// Framework7 styles
import 'framework7/css/framework7.min.css';


import {Provider} from "mobx-react";
import BackendApiService from "./services/backendApi";
import JobsStore from "./stores/JobsStore";
import DataStore from "./stores/dataStore";
import Root from "./components/Root";
import FilesStore from "./stores/FilesStore";

Framework7.use(Framework7React);

const browserHistory = createBrowserHistory(),
    routingStore = new RouterStore(),
    history = syncHistoryWithStore(browserHistory,routingStore);

const currentEnviroment = {

};
const apiService = new BackendApiService(currentEnviroment),
    jobsStore = new JobsStore(apiService),
    dataStore = new DataStore(),
    filesStore = new FilesStore(apiService);

const stores = {
    apiService,

    filesStore,
    jobsStore,
    dataStore,
    routing: routingStore,
    routingHistory: history
};

// ons.ready(() => render(
//     <div>
//         <DevTools />
//         <Provider {...stores}>
//             <React.Fragment>
//                 <Router history={history}>
//                     <Switch>
//                         <Route path="/" component={AppMainPage}/>
//                     </Switch>
//                 </Router>
//             </React.Fragment>
//         </Provider>
//     </div>,
//     document.getElementById("root")
// ));


ReactDOM.render(
    <React.Fragment>
        <DevTools />
        <Provider {...stores}>
            <Root/>
        </Provider>
    </React.Fragment>,
    document.getElementById('root'),
);
