import React from "react";
import { render } from "react-dom";
import DevTools from "mobx-react-devtools";
import {Router, Route, Switch} from "react-router";
import createBrowserHistory from 'history/createBrowserHistory';
import {RouterStore, syncHistoryWithStore } from "mobx-react-router";

import 'onsenui/css/onsenui.css';
import 'onsenui/css/onsen-css-components.css';

import {Provider} from "mobx-react";
import BackendApiService from "./services/backendApi";
import App from "./components/app";
import JobsStore from "./stores/JobsStore";

const browserHistory = createBrowserHistory(),
    routingStore = new RouterStore(),
    history = syncHistoryWithStore(browserHistory,routingStore);

const currentEnviroment = {

};
const apiService = new BackendApiService(currentEnviroment),
    jobsStore = new JobsStore(apiService);

const stores = {
    apiService,

    jobsStore,
    routing: routingStore,
    routingHistory: history
};

render(
  <div>
    <DevTools />
    <Provider {...stores}>
        <React.Fragment>
            <Router history={history}>
                <Switch>
                    <Route path="/" component={App}/>
                </Switch>
            </Router>
        </React.Fragment>
    </Provider>
  </div>,
  document.getElementById("root")
);

