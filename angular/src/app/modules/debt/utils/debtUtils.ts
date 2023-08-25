import { AbstractControl } from "@angular/forms"
import * as moment from "moment"

export const calculateTotalMoney = (debtList) => {
  return debtList.reduce((pv, cur) => {
    return pv + Number(cur.money)
  }, 0)
}

export const calculateInterest = (startDate, endDate, money, interestRate) => {
  const dayDiff = Math.ceil(Math.abs(moment(startDate).diff(moment(endDate), 'day', true)))
  if (dayDiff < 0) return 0
  const rate = (interestRate / 365) * dayDiff;
  const interest = (rate * money) / 100;
  return Math.ceil(interest)
}

export const validatePaymentDate = (start: AbstractControl, end: AbstractControl) => {
  const startDate = moment(start.value)
  const endDate = moment(end.value)
  const startMonth = startDate.month()
  const endMonth = endDate.month()
  const startYear = startDate.year();
  const endYear = endDate.year();
  const startDay = endDate.day();
  const endDay = endDate.day();
  if (startYear < endYear) {
    start.setErrors(null);
    end.setErrors(null)
    return null;
  }
  if (startYear === endYear) {
    if (startMonth > endMonth) {
      start.setErrors({ invalidDate: true })
      end.setErrors({ invalidDate: true })
      return { notValid: true }
    }
    if(startMonth == endMonth) {
      if(startDay > endDay){
        start.setErrors({ invalidDate: true })
        end.setErrors({ invalidDate: true })
        return { notValid: true }
      }
    }
    start.setErrors(null);
    end.setErrors(null)
    return null
  }
  if (startYear > endYear) {
    start.setErrors({ invalidDate: true })
    end.setErrors({ invalidDate: true })
    return { notValid: true }
  }
}
