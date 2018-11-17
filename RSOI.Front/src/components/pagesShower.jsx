import React from "react";
import PropTypes from "prop-types";
import Job from "../models/job";
import {inject, observer} from "mobx-react";
import {PhotoBrowser} from "framework7-react";
import {computed, action, observable} from "mobx";
import BackendApiService from "../services/backendApi";

@inject('apiService')
@observer
export default class PageShower extends React.Component{
    static propTypes = {
        job: PropTypes.instanceOf(Job),
        apiService: PropTypes.instanceOf(BackendApiService),
        refLink: PropTypes.func
    };


    constructor(props){
        super(props);
        this._pages = [];
    }

    componentDidUpdate(){
        for(let i = 0 ;i<this.props.job.pageCount; ++i){
            this.pb.props.photos.push(this.props.apiService.API.Files.getImageURLWithHost(this.props.job.jobId,i))
        }
    }

    render() {
        return (
            <PhotoBrowser
                photos={this._pages}
                theme="dark"
                type="page"
                backLinkText="Back"
                ref={(el) => {this.pb = el; this.props.refLink(el)}}
            />
        );
    }

}