import * as React from "react";

import "./probestats.css";

export interface ProbeRunStats {
    start: Date;
    end: Date;
    duration: number;
}

export const ProbeStats: React.SFC<ProbeRunStats> = (props: ProbeRunStats) => {
    return (
        <div className="probe-stats">
            <p>{`Started: ${props.start.toISOString()}`}</p>
            <p>{`Finished: ${props.end.toISOString()}`}</p>
            <p>{`Duration: ${props.duration}s`}</p>
        </div>
    );
};
