import { Photo } from "./Photo"

export interface Member {
    id: number
    userName: string
    photoUrl: string
    knownAs: string
    age: number
    createdOn: string
    lastActive: string
    gender: string
    introduction: string
    interests: string
    lookingFor: string
    city: string
    country: string
    photos: Photo[]
  }