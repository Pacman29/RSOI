import React from "react";
import PropTypes from "prop-types";
import {Block, List, ListButton, Page} from "framework7-react";
import Loader from "./loader";
import PageShower from "./pagesShower";
import {inject, observer} from "mobx-react";
import {action, observable} from "mobx";
import JobsStore from "../stores/JobsStore";
import Job from "../models/job";
import AppNavbar from "./appNavbar";

@inject("jobsStore")
@observer
export default class UpdateJobPage extends React.Component {
    static propTypes = {
        jobsStore: PropTypes.instanceOf(JobsStore),
        job: PropTypes.instanceOf(Job)
    };

    @observable file;

    constructor(props){
        super(props);
        this.jobFilesStore = this.props.jobsStore.getStoreForJobId(this.props.job.jobId);
    }

    @action handleFileInputChange = (evt) => {
        this.file = evt.target.files[0] || undefined;
    };

    @action handleUploadClick = (evt) => {
        if(this.file)
            this.jobFilesStore.updateJob(this.file)
                .then(action(res => {
                    this.$f7router.navigate(`/`);
                }))
    };

    render(){
        return (
            <Page>
                <AppNavbar back={true}/>
                <Loader isLoading={this.jobFilesStore.isLoading}>
                    <Block >
                        <div style={{display: "flex",justifyContent: "space-between"}}>
                            <span>Job ID:</span>
                            <span>{this.props.job.jobId}</span>
                        </div>
                        <List inset>
                            <div style={{display: "flex", justifyContent: "center"}}>
                                <input className="button button-big" type='file' id='_file'
                                       accept="application/pdf" onChange={this.handleFileInputChange}/>
                            </div>
                            <ListButton title="Update" onClick={this.handleUploadClick}/>
                        </List>
                    </Block>
                </Loader>
            </Page>
        );
    }
}