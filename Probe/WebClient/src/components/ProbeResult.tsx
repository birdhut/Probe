import * as React from "react";
import Loader from "./Loader";

import "./probeResult.css";

interface ProbeResultProps {
    id: string;
    result?: any;
    isLoading?: boolean;
}

const ProbeResult: React.SFC<ProbeResultProps> = (props: ProbeResultProps) => {
    if (!props.isLoading && !props.result) {
        return null;
    }

    return (
        <pre key={`probe-result-${props.id}`}>
            {props.isLoading && <Loader />}
            {props.result && JSON.stringify(props.result, null, 2)}
        </pre>
    );
} 

export default ProbeResult;
