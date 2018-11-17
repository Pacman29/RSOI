import React from "react";
import {Component} from "react";
import {inject, observer} from "mobx-react";
import {action, computed, observable} from "mobx";
import {RouterStore} from "mobx-react-router";
import PropTypes from "prop-types";
import JobsStore from "../stores/JobsStore";
import Loader from "./loader";
import DataStore from "../stores/dataStore";
import JobsList from "./jobsList";
import {Page, Navbar, Block, Fab} from "framework7-react";
import {Icon} from "framework7-react";

@inject('routing')
@inject('jobsStore')
@inject('dataStore')
@observer
export default class AppMainPage extends Component {
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

    @action loadAllJobs = () =>{
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

    @action reloadJobs = (evt,done) => {
        this.loadAllJobs();
        done();
    };

    @action handleOnCreateJobClick = (evt) => {
        this.$f7router.navigate(`/createJob`);
    };

    render(){
        return (
            <Page ptr onPtrRefresh={this.reloadJobs}>
                <Navbar title="Recognize Service"/>
                    <Loader isLoading={this.props.jobsStore.isLoading}>
                        <JobsList jobs={this.jobs}/>
                    </Loader>
                <Fab position="right-bottom" slot="fixed" color="orange" onClick={this.handleOnCreateJobClick}>
                    <Icon ios="f7:add" md="material:add"></Icon>
                </Fab>
            </Page>
        );
    }
}