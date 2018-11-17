import React from "react";
import PropTypes from "prop-types";
import {Block, List, ListButton, Navbar, Page, ListInput} from "framework7-react";
import Loader from "./loader";
import PageShower from "./pagesShower";
import {inject, observer} from "mobx-react";
import {action, observable} from "mobx";
import JobsStore from "../stores/JobsStore";

@inject("jobsStore")
@observer
export default class CreateJobPage extends React.Component {
    static propTypes = {
        jobsStore: PropTypes.instanceOf(JobsStore)
    };

    @observable file;

    constructor(props){
        super(props)
    }

    @action handleFileInputChange = (evt) => {
        this.file = evt.target.files[0] || undefined;
    };

    @action handleUploadClick = (evt) => {
        if(this.file)
            this.props.jobsStore.createJob(this.file)
                .then(action(res => {
                    this.$f7router.navigate(`/`);
                }))
    };

    render(){
        return (
            <Page>
                <Navbar title="Recognize Service" backLink="Back"/>
                <Loader isLoading={this.props.jobsStore.isLoading}>
                    <Block >
                        <List inset>
                            <div style={{display: "flex", justifyContent: "center"}}>
                                <input className="button button-big" type='file' id='_file'
                                    accept="application/pdf" onChange={this.handleFileInputChange}/>
                            </div>
                            <ListButton title="Upload" onClick={this.handleUploadClick}/>
                        </List>
                    </Block>
                </Loader>
            </Page>
        );
    }
}