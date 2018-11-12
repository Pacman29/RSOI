import React from "react";
import {Component} from "react";
import {inject, observer} from "mobx-react";
import {RouterStore} from "mobx-react-router";
import PropTypes from "prop-types";
import {Button, Page, Toolbar} from "react-onsenui";
import JobsStore from "../stores/JobsStore";

@inject('routing')
@inject('jobsStore')
@observer
export default class App extends Component {
    static propTypes = {
        routing: PropTypes.instanceOf(RouterStore),
        jobsStore: PropTypes.instanceOf(JobsStore)
    };

    constructor(props) {
        super(props);
    }

    componentDidMount(){
        this.props.jobsStore.getAllJobs().then(res => console.log(res));
    }

    render(){
        return (
            <Page>
                <Toolbar>
                    <div className="center">Recognize Service</div>
                </Toolbar>

                <Button>Click me!</Button>
            </Page>
        );
    }
}