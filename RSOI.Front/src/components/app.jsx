import React from "react";
import {Component} from "react";
import {inject, observer} from "mobx-react";
import {action, computed, observable} from "mobx";
import {RouterStore} from "mobx-react-router";
import PropTypes from "prop-types";
import {Button, Page, Toolbar} from "react-onsenui";
import JobsStore from "../stores/JobsStore";
import Loader from "./loader";
import DataStore from "../stores/dataStore";
import MainPage from "./mainPage";

@inject('routing')
@inject('jobsStore')
@inject('dataStore')
@observer
export default class App extends Component {
    static propTypes = {
        routing: PropTypes.instanceOf(RouterStore),
        jobsStore: PropTypes.instanceOf(JobsStore),
        dataStore: PropTypes.instanceOf(DataStore)
    };

    @observable _jobs = [];

    constructor(props) {
        super(props);
    }

    componentDidMount(){
        this.loadAllJobs();
    }

    @action loadAllJobs(){
        this.props.jobsStore.getAllJobs()
            .then(action(res => {
                this.props.dataStore.createOrUpdateJobs(res);
                this._jobs = this.props.dataStore.getAllJobs();

            }))
            .catch(action(err => console.log(err)));
    }

    @computed get jobs(){
        return this._jobs;
    }

    render(){
        return (
            <Page renderToolbar={() =>(
                <Toolbar>
                    <div className="center">Recognize Service</div>
                </Toolbar>
            )}>

                <Loader isLoading={this.props.jobsStore.isLoading}>
                    <MainPage jobs={this.jobs}/>
                </Loader>
            </Page>
        );
    }
}