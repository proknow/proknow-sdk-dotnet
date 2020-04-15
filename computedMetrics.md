# Computed Metrics

## Instructions

The following *Metric List* identifies the types of supported metrics and the required arguments.

For example, in order to compose a metric for "Dose (Gy) covering specified volume (%) of the specified structure,"
you need to specify *roiName* and *arg1*:

```
var computedMetric = new ComputedMetric("DOSE_VOLUME_PERCENT_ROI", "PTV", 99);
```

Here's another example with "Volume (%) of the specified structure covered by specified dose (Gy)" that uses *roiName*,
*arg1*, and *arg2*:

```
var computedMetric = new ComputedMetric("VOLUME_PERCENT_DOSE_RANGE_ROI", "BRAINSTEM", 0, 10);
```

Some computed metric types may not require any arguments at all:

```
var computedMetric = new ComputedMetric("MAX_DOSE");
```

## Metric List

* Dose (Gy) covering specified volume (%) of the specified structure

  * Type: ``DOSE_VOLUME_PERCENT_ROI``
  * Arguments: Dose (Gy) covering *arg1* (%) of the *roiName*

* Dose (Gy) covering specified volume (cc) of the specified structure

  * Type: ``DOSE_VOLUME_CC_ROI``
  * Arguments: Dose (Gy) covering *arg1* (cc) of the *roiName*

* Dose (Gy) covering the specified structure's total volume (cc) minus specified volume (cc)

  * Type: ``DOSE_VOLUME_MINUS_CC_ROI``
  * Arguments: Dose (Gy) covering the total volume (cc) minus *arg1* (cc) of the *roiName*

* Volume (cc) of the specified structure covered by specified dose (Gy)

  * Type: ``VOLUME_CC_DOSE_ROI``
  * Arguments: Volume (cc) of the *roiName* covered by *arg1*

* Volume (%) of the specified structure covered by specified dose (Gy)

  * Type: ``VOLUME_PERCENT_DOSE_ROI``
  * Arguments: Volume (%) of the *roiName* covered by *arg1*

* Volume (cc) of the specified structure in specified dose range (Gy)

  * Type: ``VOLUME_CC_DOSE_RANGE_ROI``
  * Arguments: Volume (cc) of the *roiName* in range of *arg1* to *arg2* (Gy)

* Volume (%) of the specified structure in specified dose range (Gy)

  * Type: ``VOLUME_PERCENT_DOSE_RANGE_ROI``
  * Arguments: Volume (%) of the *roiName* in range of *arg1* to *arg2* (Gy)

* Minimum dose (Gy) to the specified structure

  * Type: ``MIN_DOSE_ROI``
  * Arguments: Minimum dose (Gy) to the *roiName*

* Maximum dose (Gy) to the specified structure

  * Type: ``MAX_DOSE_ROI``
  * Arguments: Maximum dose (Gy) to the *roiName*

* Mean dose (Gy) to the specified structure

  * Type: ``MEAN_DOSE_ROI``
  * Arguments: Mean dose (Gy) to the *roiName*

* Integral dose (Gy |middot| cc) to the specified structure

  * Type: ``INTEGRAL_DOSE_ROI``
  * Arguments: Integral dose (Gy |middot| cc) to the *roiName*

* Global maximum dose (Gy) over the entire dose grid

  * Type: ``MAX_DOSE``
  * Arguments: Global maximum dose (Gy) over the entire dose grid

* Volume of Regret

  * Type: ``VOLUME_OF_REGRET``
  * Arguments: Total volume (cc) covered by *arg1* (Gy) but outside of the *roiName*

* Irradiated Volume

  * Type: ``IRRADIATED_VOLUME``
  * Arguments: Total volume (cc) covered by *arg1*

* Conformation Number

  * Type: ``CONFORMATION_NUMBER``
  * Arguments: Conformation number of *roiName* at *arg1*

* Conformality Index

  * Type: ``CONFORMALITY_INDEX``
  * Arguments: Conformality index of *roiName* at *arg1*

* Homogeneity Index

  * Type: ``HOMOGENEITY_INDEX``
  * Arguments: Homogeneity index of *roiName* at *arg1*

* Inhomogeneity Index

  * Type: ``INHOMOGENEITY_INDEX``
  * Arguments: Inhomogeneity index of *roiName*

* Cumulative meterset for all associated treatment beams

  * Type: ``CUMULATIVE_METERSET``
  * Arguments: Cumulative meterset

* Volume (cc) of the specified structure

  * Type: ``VOLUME``
  * Arguments: Volume (cc) of the *roiName*